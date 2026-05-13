using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Domain.Entities;
using MeetInSport.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using MeetInSport.Infrastructure.Persistance;

namespace MeetInSport.Infrastructure.Persistence.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{

    public ReservationRepository(AppDbContext context) : base(context)
    { }

    public async Task<IReadOnlyList<Reservation>> GetReservationsByUserIdAsync(Guid userId)
    {
        return await _dbSet
        .Include(c => c.Coach).ThenInclude(u => u.User)
        .Include(p => p.Package)
        .Include(s => s.Student)
        .Where(u => u.StudentId == userId).ToListAsync();
        
    }
    public async Task<IReadOnlyList<Reservation>> GetReservationsByCoachIdAsync(Guid coachId)
    {
        return await _dbSet
        .Include(r => r.Coach).ThenInclude(u => u.User)
        .Include(p => p.Package)
        .Include(r => r.Student)
        .Where(c => c.CoachId == coachId).ToListAsync();
    }
}