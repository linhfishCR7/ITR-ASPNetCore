using Microsoft.EntityFrameworkCore;

namespace UploadS3.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<FileEntry> FileEntries => Set<FileEntry>();

    }
}