using AutoMapper;
using MeetInSport.Application.DTOs.Reservation;
using MeetInSport.Application.Exceptions;
using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Domain.Entities;
using MeetInSport.Domain.Enum;


namespace MeetInSport.Application.Services;

public class ReservationService : IReservationService
{

    private readonly IReservationRepository _reservationRepository;
    private readonly IGenericRepository<LessonPackage> _packageRepository;
    private readonly IMapper _mapper;
    private readonly ICoachRepository _coachRepository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public ReservationService(
        IReservationRepository reservationRepository,
        IGenericRepository<LessonPackage> packageRepository,
        IMapper mapper,
        ICoachRepository coachRepository,
        IEmailService emailService,
        IUserRepository userRepository
      )
    {
        _reservationRepository = reservationRepository;
        _packageRepository = packageRepository;
        _mapper = mapper;
        _coachRepository = coachRepository;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<ReservationResponseDto> ConfirmReservationAsync(Guid reservationId, Guid userId, string role){
        var reservation = await _reservationRepository.GetByIdAsync(reservationId)
        ?? throw new NotFoundException(nameof(Domain.Entities.Reservation), reservationId);

        if(role == "Coach"){
            var coach = await _coachRepository.GetCoachByUserIdAsync(userId);
            if(coach == null || reservation.CoachId != coach.Id){
                throw new UnauthorizedAccessException("Bu rezervasyonu onaylamaya yetkiniz bulunmamaktadır.");
            }
        }
        else{
            throw new UnauthorizedAccessException("Sadece Antrenörler Rezervasyonu Onaylayabilir.");
        }
        if(reservation.Status != ReservationStatus.Pending){
            throw new InvalidOperationException($"Rezervasyon Onaylanamaz. Durum : {reservation.Status}.");
        }
        reservation.Status = ReservationStatus.Confirmed;
        reservation.UpdatedAt = DateTime.UtcNow;

        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync();
        
        // Sending mail to student

        try{
            var studentUser = await _userRepository.GetByIdAsync(reservation.StudentId);
            if(studentUser != null && !string.IsNullOrEmpty(studentUser.Email)){
                string subject = "Rezervasyonunuz antrenörünüz tarafından onaylandı. - MeetInSport";
                string body = $"Merhaba {studentUser.Name}, \n\n" + 
                $"Harika bir haber! Antrenörünüz ders reervasyonunuzu onayladı. \n" +
                $"Tarih : {reservation.ScheduledAt.ToString("dd/MM/yyyyHH:mm")} \n\n" +
                $"Konum: {reservation.LocationType} \n\n" +
                $"Eğer herhangi bir sorun yaşanırsa, lütfen en kısa sürede antrenörünüzle iletişime geçin.İyi antrenmanlar dileriz :)\n\n" +
                $"Saygılarımızla, \nMeetInSport Ekibi";
            
            await _emailService.SendEmailAsync(studentUser.Email, subject, body);            
            }
        }
        catch(Exception ex){
            Console.WriteLine($"Rezervasyon onay email gönderilemedi: {ex.Message}");
        }
        return _mapper.Map<ReservationResponseDto>(reservation);
    }


    public async Task<ReservationResponseDto> CancelReservationAsync(Guid reservationId, Guid userId, string role, CancelReservationDto cancelReservationDto)
    {
       var reservation = await _reservationRepository.GetReservationWithDetailsByIdAsync(reservationId) ?? throw new NotFoundException(nameof(Domain.Entities.Reservation), reservationId);

        if (role == "Coach")
        {
            var coach = await _coachRepository.GetCoachByUserIdAsync(userId);
            if (coach == null || reservation.CoachId != coach.Id)
            {
                throw new UnauthorizedAccessException("Bu rezervasyonu iptal etmeye yetkiniz bulunmamaktadır.");
            }
        }
        else
        {
            if (reservation.StudentId != userId)
            {
                throw new UnauthorizedAccessException("Bu rezervasyonu iptal etmeye yetkiniz bulunmamaktadır.");
            }
        }

        if (reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Completed)
        {
            throw new InvalidOperationException($"Rezervasyon iptal edilemez. Durum : {reservation.Status}.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTime.UtcNow;
        reservation.CancelReason = cancelReservationDto.CancelReason;
        reservation.UpdatedAt = DateTime.UtcNow;

        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync();
        
        try {
            if(role == "Coach" && reservation.Student != null){
                var subject = "Rezervasyon İptali Bilgilendirmesi";
                var body = $"Merhaba {reservation.Student.Name},\n\n" +
                           $"Antrenörünüz '{reservation.Package?.PackageName}' ders paketine ait rezervasyonunuzu maalesef iptal etmiştir.\n" +
                           $"İptal Nedeni: {cancelReservationDto.CancelReason}\n\n" +
                           $"Detaylı bilgi için antrenörünüzle iletişime geçebilirsiniz.\n\n" +
                           $"Saygılarımızla,\nMeetInSport Ekibi";
            
            await _emailService.SendEmailAsync(reservation.Student.Email, subject,body);
            }
            else if (role == "Student" && reservation.Coach?.User != null){
                var subject = "Öğrenci Rezervasyon İptali";
                var body = $"Merhaba {reservation.Coach.User.Name},\n\n" +
                           $"Öğrenciniz '{reservation.Package?.PackageName}' ders paketine ait rezervasyonunu iptal etmiştir.\n" +
                           $"İptal Nedeni: {cancelReservationDto.CancelReason}\n\n" +
                           $"Saygılarımızla,\nMeetInSport Ekibi";
                    await _emailService.SendEmailAsync(reservation.Coach.User.Email, subject, body);
            }
        }
        catch(Exception ex) {
            Console.WriteLine($"Rezervasyon İptali Email Gönderilemedi : {ex.Message}");
        }
        return _mapper.Map<ReservationResponseDto>(reservation);
    }

    public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto createReservationDto, Guid studentId)
    {
        // Verify whether the package is exit or not;
        var requiredPackage = await _packageRepository.GetByIdAsync(createReservationDto.PackageId) ?? throw new NotFoundException(nameof(LessonPackage), createReservationDto.PackageId);

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            PackageId = requiredPackage.Id,
            CoachId = requiredPackage.CoachId,
            ScheduledAt = createReservationDto.ScheduleAt.ToUniversalTime(),
            LocationType = createReservationDto.LocationType,
            Notes = createReservationDto.Notes,
            Status = ReservationStatus.Pending, // all new reservations start as panding
            CreatedAt = DateTime.UtcNow,
            ExpirationAt = DateTime.UtcNow.AddDays(requiredPackage.ExpirationDays),
        };
        await _reservationRepository.AddAsync(reservation);
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationResponseDto>(reservation);
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetMyReservationsAsync(Guid userId, string role)
    {
        IReadOnlyList<Reservation> reservations;

        if (role == "Coach")
        {
            // CoachId was found first
            var coach = await _coachRepository.GetCoachByUserIdAsync(userId) ?? throw new NotFoundException("Coach Profile,", userId);
            reservations = await _reservationRepository.GetReservationsByCoachIdAsync(coach.Id);
        }
        else
        {
            reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
        }
        return _mapper.Map<IEnumerable<ReservationResponseDto>>(reservations);
    }
    public async Task<ReservationResponseDto> GetReservationByIdAsync(Guid reservationId, Guid userId, string role){
        var reservation = await _reservationRepository.GetReservationWithDetailsByIdAsync(reservationId);

        if(reservation == null){
            throw new NotFoundException(nameof(Reservation), reservationId);
        }
        if(role == "Student" && reservation.StudentId != userId){
            throw new UnauthorizedAccessException("Bu rezervasyonu görmeye yetkiniz bulunmamaktadır.");
        }
        if(role == "Coach"){
            var coach = await _coachRepository.GetCoachByUserIdAsync(userId) ?? throw new NotFoundException("Coach Profile", userId);
            if(reservation.CoachId != coach.Id){
                throw new UnauthorizedAccessException("Bu rezervasyonu görmeye yetkiniz bulunmamaktadır.");
            }
        }
        return _mapper.Map<ReservationResponseDto>(reservation);
    }
}