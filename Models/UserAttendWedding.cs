using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class UserAttendWedding // Many User : Many Wedding
    {
        [Key]
        public int UserAttendWeddingId{get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public int UserId {get;set;}
        public User User {get;set;} // User who's attending
        public int WeddingId {get;set;}
        public Wedding Wedding {get;set;} // Wedding that the User is attending
    }
}