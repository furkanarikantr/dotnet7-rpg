using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Data
{   
    //DataContext => Veri bağlama sınıfı. Veritabanı verilerini soyut bir şekilde getirir, bu sayede güncelleme ekleme gibi HTTP methodları sayesinde değişiklikler yapabiliriz.
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<Character> Characters => Set<Character>();
    }
}