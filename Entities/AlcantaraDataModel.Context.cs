﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlcantaraClient.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CourseWorks_ThirdCourse_AlcantaraDbEntities : DbContext
    {
        public static CourseWorks_ThirdCourse_AlcantaraDbEntities Instance { get; private set; } = new CourseWorks_ThirdCourse_AlcantaraDbEntities();

        private CourseWorks_ThirdCourse_AlcantaraDbEntities()
            : base("name=CourseWorks_ThirdCourse_AlcantaraDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Basket> Baskets { get; set; }
        public virtual DbSet<CategoryReference> CategoryReferences { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Firm> Firms { get; set; }
        public virtual DbSet<Furniture> Furnitures { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}