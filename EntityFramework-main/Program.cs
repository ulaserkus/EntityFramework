using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using entityframework.Data.EfCore;

namespace entityframework
{
    // entity classes
    // Product(Id,Name)=>Product(Id,Name)
    public class ShopContext:DbContext
    {
        public DbSet<Product> Products{get; set;}
        public DbSet<Category> Categories {get; set;}
        public DbSet<User> Users { get; set; }
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                   .UseLoggerFactory(MyLoggerFactory)
                   //.UseSqlite("Data Source=Shop.db");
                   .UseMySql("server=localhost;port=xxxx;database=ShopDb;user=root;password=xxxxxx;",

                   new MySqlServerVersion(new Version(8, 0, 21)), // use MariaDbServerVersion for MariaDB
                        mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend));

                   

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()
                         .HasKey(t => new { t.ProductId,t.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);


            modelBuilder.Entity<ProductCategory>()
               .HasOne(pc => pc.Category)
               .WithMany(c => c.ProductCategories)
               .HasForeignKey(pc => pc.CategoryId);
        }

    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public Customer Customer { get; set; }

        public Supplier Supplier { get; set; }

        public List<Adress> Adresses { get; set; }


    }

    public class Customer
    {
        public int Id { get; set; }
        public string IdentityNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

    }

    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string TaxNumber{ get; set; }


        public User User { get; set; }
        public int UserId { get; set; }

    }
    public class Adress
    {

        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }


        public User User { get; set; }
        public int UserId { get; set; }

    }

    //convention 
    //data annotions
    //fluent api => many to many
    public class Product
    {
        //primary key(Id,<type_name>Id)
       
        public int Id { get; set; }


        public string Name { get; set; }


        public decimal Price { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }

    }

    public class Category
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }

    }

    public class ProductCategory
    {

        public int  ProductId { get; set; }
        public Product Product { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }

 
    class Program
    {
        static void Main(string[] args)
        {

            using (var db = new NorthwindContext())
            {

                var products =  db.Products.ToList();

                foreach (var p in products)
                {
                    Console.WriteLine(p.ProductName);
                }
            }
            

           
        }

        static void InsertUsers()
        {
            var users = new List<User>
            {
                new User(){Username = "UlasErkus",Email="ulaserkus99@gmail.com"},
                new User(){Username = "HasanTahsin",Email="HasanTahsin@gmail.com"},
                new User(){Username = "AhmetSahin",Email="ahmeteren99@gmail.com"},
                new User(){Username = "HakanYalcınkaya",Email="hakanyalcinkaya@gmail.com"}
            };


            using (var db = new ShopContext()) {


                db.Users.AddRange(users);
                db.SaveChanges();


            }
        }
        static void InsertAdress()
        {
            var adresses = new List<Adress>
            {
                new Adress(){Fullname = "ulas erkus",Title="ev adresi",Body="istanbul",UserId=1},
                new Adress(){Fullname = "ulas erkus",Title="iş adresi",Body="istanbul",UserId=1},
                new Adress(){Fullname = "hasan tahsin",Title="iş adresi",Body="ankara",UserId=2},
                new Adress(){Fullname = "ahmet eren sahin",Title="ev adresi",Body="bursa",UserId=3},
                new Adress(){Fullname = "hakan yalçınkaya",Title="ev adresi",Body="istanbul",UserId=4},
                new Adress(){Fullname = "hakan yalçınkaya",Title="iş adresi",Body="istanbul",UserId=4}

            };


            using (var db = new ShopContext())
            {


                db.Adresses.AddRange(adresses);
                db.SaveChanges();


            }
        }

        static void DeleteProduct(int id)
        {

            using (var db = new ShopContext())
            {
                var p = db.Products.FirstOrDefault(i => i.Id == id);
                if(p != null)
                {
                    db.Products.Remove(p);

                    db.SaveChanges();
                    Console.WriteLine("veri silindi");
                }

            }
        }
        static void UpdateVersionTwo(int id)
        {
            using (var db = new ShopContext())
            {
                var entity = new Product { Id = id };

                db.Products.Attach(entity);
                entity.Price = 3000;
                db.SaveChanges();
            }
        }
        static void UpdateProduct(int id) {

            using (var context = new ShopContext())
            {

                // change tracking .AsNoTracking()
                var product = context.Products.Where(i => i.Id == id).FirstOrDefault();
                if(product != null)
                {
                    product.Price *= 1.2m;
                    context.SaveChanges();
                    Console.WriteLine("güncelleme yapıldı");
                }


            }


        }

        static void GetProductById(int Id)
        {
            using (var context = new ShopContext())
            {
              

                var product = context.Products.Where(p => p.Id == Id).FirstOrDefault();

                Console.WriteLine($"Name : {product.Name} Price : {product.Price}");
            }
        }

        static void GetAllProducts(){

            using (var context = new ShopContext())
            {

                var products = context.Products.Select(product => new {product.Name,product.Price }).ToList();

                foreach (var p in products)
                {
                    Console.WriteLine($"Name : {p.Name} Price : {p.Price}" );
                }
            }
            
        }


        static void AddProducts()
        {
            using (var db = new ShopContext())
            {

                var products = new List<Product>()
                {

                    new Product { Name = "Samsung S5", Price = 2000 },
                    new Product { Name = "Samsung S6", Price = 3000 },
                    new Product { Name = "Samsung S7", Price = 4000 },
                    new Product { Name = "Samsung S8", Price = 5000 },
                    new Product { Name = "Samsung S9", Price = 6000 },
                };

                db.Products.AddRange(products);

                db.SaveChanges();


                Console.WriteLine("veriler eklendi");




            }
        }


        static void AddProduct()
        {
            using (var db = new ShopContext())
            {

                var product = new Product { Name = "Samsung S10", Price = 8000 };

                db.Products.Add(product);

                db.SaveChanges();


                Console.WriteLine("veriler eklendi");




            }
        }
    }
}
