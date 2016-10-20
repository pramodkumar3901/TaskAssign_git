using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskAssign.Models
{
    public class HistoryModels
    {
    }

    public class HistoryList
    {

        [Display(Name = "Histroy Id")]
        public int HistoryId { get; set; }

        [Required]
        [Display(Name = "Task Id")]
        public int TaskId { get; set; }


        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}