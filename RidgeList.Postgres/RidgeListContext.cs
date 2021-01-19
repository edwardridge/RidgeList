// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using RidgeList.Domain;
//
// namespace RidgeList.Postgres
// {
//     public class RidgeListContext : DbContext
//     {
//         public RidgeListContext()
//         {
//             this.ChangeTracker.LazyLoadingEnabled = false;
//         }
//         
//         public DbSet<Wishlist> Wishlists { get; set; }
//
//         
//         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//         {
//             optionsBuilder.EnableSensitiveDataLogging(true);
//             optionsBuilder.UseNpgsql(
//                 @"Username=postgres;Password=local;Host=localhost;Port=5432;Database=postgres;Include Error Detail=true");
//         }
//
//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             modelBuilder.Entity<WishlistPerson>().HasKey(s => s.Id);            
//             // modelBuilder.Entity<Wishlist>().HasKey(s => s.Id);
//             modelBuilder.Entity<PresentIdea>().HasKey(s => s.Id);
//             modelBuilder.Entity<PresentIdea>().HasOne(s => s.WishlistPerson).WithMany(s => s.PresentIdeas).HasForeignKey(s => s.Id);
//         }
//     }
//
//     public class EfWishlistRepository : IWishlistRepository
//     {
//         private readonly RidgeListContext _context;
//
//         public EfWishlistRepository(RidgeListContext context)
//         {
//             _context = context;
//         }
//         
//         public async Task Save(Wishlist wishlist)
//         {
//             if (wishlist.Id == Guid.Empty)
//             {
//                 this._context.Wishlists.Add(wishlist);
//             }
//             else
//             {
//                 this._context.Wishlists.Update(wishlist);
//             }
//             _context.SaveChanges();
//         }
//
//         public async Task Update(Wishlist wishlist)
//         {
//             try
//             {
//                 this._context.Update(wishlist);
//                 await this._context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException ex)
//             {
//                 foreach (var entry in ex.Entries)
//                 {
//                     if (entry.Entity is PresentIdea)
//                     {
//                         var proposedValues = entry.CurrentValues;
//                         var databaseValues = entry.GetDatabaseValues();
//
//                         foreach (var property in proposedValues.Properties)
//                         {
//                             var proposedValue = proposedValues[property];
//                             var databaseValue = databaseValues[property];
//
//                             // TODO: decide which value should be written to database
//                             // proposedValues[property] = <value to be saved>;
//                         }
//
//                         // Refresh original values to bypass next concurrency check
//                         entry.OriginalValues.SetValues(databaseValues);
//                     }
//                     else
//                     {
//                         throw new NotSupportedException(
//                             "Don't know how to handle concurrency conflicts for "
//                             + entry.Metadata.Name);
//                     }
//                 }
//             }
//
//             
//         }
//
//         public Task<Wishlist> Load(Guid id)
//         {
//             return Task.FromResult(this._context.Wishlists.Include(s => s.People).ThenInclude(s => s.PresentIdeas).FirstOrDefault(s => s.Id == id));
//         }
//
//         public Task<IEnumerable<WishlistSummary>> GetWishlistSummaries(string emailAddress)
//         {
//             var ffff = this._context.Wishlists.Where(s => s.People.Any(s => s.Email == emailAddress));
//             List<WishlistSummary> wishlistSummaries = new List<WishlistSummary>();
//             foreach (var f in ffff)
//             {
//                 wishlistSummaries.Add(new WishlistSummary()
//                 {
//                     Id = f.Id,
//                     Name = f.Name
//                 });
//             }
//                 
//             return Task.FromResult(wishlistSummaries as IEnumerable<WishlistSummary>);
//         }
//
//         public async Task Delete(Guid id)
//         {
//             var f = await this.Load(id);
//             _context.Wishlists.Remove(f);
//             await this._context.SaveChangesAsync();
//         }
//     } 
// }