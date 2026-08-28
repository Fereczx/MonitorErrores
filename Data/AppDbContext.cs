using Microsoft.EntityFrameworkCore;
using MonitorErrores.Models;

namespace MonitorErrores.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Error> Errores { get; set; }

    public DbSet<Diagnostico> Diagnosticos { get; set; }
}