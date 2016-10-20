using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaskAssign.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<History> Histories { get; set; }
       
    }

    [Table("Member")]
    public class Member
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool Active { get; set; }
    }

    [Table("Lookup")]
    public class Lookup
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LookupId { get; set; }
        public string LookupName { get; set; }
        public string LookupType { get; set; }
        public int LookupOrder { get; set; }
        public int ParentId { get; set; }
    }


    [Table("Task")]
    public class Task
    {
        public Task()
        {
            RequestFlag = false;
            RequestHours = 0;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        public int TaskType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime PostedDate { get; set; }

        public TimeSpan PostedTime { get; set; }
        public int PostedBy { get; set; }
        public int AssignedBy { get; set; }
        public string ModuleName { get; set; }
        public int Priority { get; set; }
        public int AssignedTo { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime AssignedOn { get; set; }
        public int AllottedHrs { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime DueDate { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime StartedDate { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime ClosedDate { get; set; }

        public int DevStatus { get; set; }
        public int InitStatus { get; set; }
        public int TesterStatus { get; set; }
        public string RejectReason { get; set; }
        public string Comments { get; set; }


        public bool RequestFlag { get; set; }

        public int RequestHours { get;set; }

        public string RequestReason { get; set; }
    }

    [Table("History")]
    public class History
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int HistoryId { get; set; }
        public int TaskId { get; set; }
        public string Messages { get; set; }
    }
   
}