using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}

        [Required(ErrorMessage = "is required")]
        [MinLength(2, ErrorMessage = "must be more than 2 characters.")]
        [MaxLength(45, ErrorMessage = "must not be more than 45 characters.")]
        [Display(Name = "Wedder One")]
        public string WedderOne {get;set;}
        
        [Required(ErrorMessage = "is required")]
        [MinLength(2, ErrorMessage = "must be more than 2 characters.")]
        [MaxLength(45, ErrorMessage = "must not be more than 45 characters.")]
        [Display(Name = "Wedder Two")]
        public string WedderTwo {get;set;}

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "is required")]
        [RestrictedDate(ErrorMessage = "must be future date.")]
        public DateTime Date {get;set;}
        
        [Required(ErrorMessage = "is required")]
        [MinLength(2, ErrorMessage = "must be more than 2 characters.")]
        [MaxLength(45, ErrorMessage = "must not be more than 45 characters.")]
        [Display(Name = "Wedding Address")]
        public string Address {get;set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        
        // Relationship properties: foreign keys and navigation properties
        // Navigation properties are null unless .Include is used
        // "Object reference not set to an instase of an object" error will show up if accessed but not included
        
        // 1 user can create many weddings
        // Foreign key is always placed on the many for a one to many relationship
        public int UserId {get;set;}
        public User CreatedBy {get;set;} // corresponding data type to the UserId

        // Many User : Many Wedding
        public List<UserAttendWedding> Attending {get;set;}
    }
}