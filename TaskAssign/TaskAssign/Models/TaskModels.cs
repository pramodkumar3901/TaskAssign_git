using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace TaskAssign.Models
{
    public class TaskModels
    { 
    
    }
    public class TaskList
    {
        [Required]
        [Display(Name = "Task Id")]
        public int TaskId { get; set; }

        [Required]
        [Display(Name = "Task Type")]
        public int TaskType { get; set; }

        
        [Display(Name = "Task Type")]
        public string TaskTypeName { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Posted Date")]
        public DateTime PostedDate { get; set; }

        [Required]
        [Display(Name = "Posted Time")]
        public TimeSpan PostedTime { get; set; }

        [Required]
        [Display(Name = "Posted By")]
        public int PostedBy { get; set; }

       
        [Display(Name = "Posted By")]
        public string PostedByName { get; set; }

        [Required]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        [Required]
        [Display(Name = "Priority")]
        public int Priority { get; set; }

        
        [Display(Name = "Priority")]
        public string PriorityName { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public int AssignedTo { get; set; }

        public string AssignedToName { get; set; }

        [Required]
        [Display(Name = "Assigned On")]
        public DateTime AssignedOn { get; set; }

        [Required]
        [Display(Name = "Allotted Hours")]
        public int AllottedHrs { get; set; }

        [Required]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Init Status")]
        public int InitStatus { get; set; }

        
        public string InitStausName { get; set; }

        [Required]
        [Display(Name = "Developer Status")]
        public int DevStatus { get; set; }

        public string DevStausName { get; set; }

        [Required]
        [Display(Name = "Tester Status")]
        public int TesterStatus { get; set; }

        public string TesterStausName { get; set; }

        [Required]
        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }

        
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Required]
        [Display(Name = "Assigned By")]
        public int AssignedBy { get; set; }

        public string AssignedByName { get; set; }

        [Required]
        [Display(Name = "Started Date")]
        public DateTime StartedDate { get; set; }

        [Required]
        [Display(Name = "Closed Date")]
        public DateTime ClosedDate { get; set; }

        [Display(Name = "Request Flag")]
        public bool RequestFlag { get; set; }

        [Required]
        [Display(Name = "Request Hours")]
        public int RequestHours { get; set; }

        [Required]
        [Display(Name = "Request Reason")]
        public string RequestReason { get; set; }

        [Display(Name = "Strict Mode")]
        public bool SctrictMode { get; set; }
    }

}