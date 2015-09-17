using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SnappetProject.Models
{
    public class Work
    {
        public int SubmittedAnswerId { get; set; }
        //[Display(Name = "Submit Date")]
        //[DisplayFormat(DataFormatString="{0:yyyy-MM-dd}")]
        //[DataType(DataType.Date)]
        public string SubmitDateTime { get; set; }

        public int Correct { get; set; }
        public int Progress { get; set; }
        public int UserId { get; set; }
        public int ExerciseId { get; set; }
        public string Difficulty { get; set; }
        public string Subject { get; set; }
        public string Domain { get; set; }
        public string LearningObjective { get; set; }

        [DataType(DataType.Date)]
        public DateTime SubmitDate
        {
            get
            {
                string date = this.SubmitDateTime.Substring(0, 10);
                DateTime submitDate = Convert.ToDateTime(date);
                return submitDate;
            }
            private set
            {
                this.SubmitDate = value;
            }
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public DateTime SubmitDate { get; set; }
        public string Subject { get; set; }
        public string Domain { get; set; }
        public int Progress { get; set; }
    }

    public class Subject
    {
        [DataType(DataType.DateTime)]
        public DateTime SubmitDate { get; set; }
        public string SubjectName { get; set; }
        public int Progress { get; set; }
    }

    //public class Subject
    //{
    //    public string SubjectName { get; set; }
    //    public List<Work> recordsList{get;set;}
    //}

}