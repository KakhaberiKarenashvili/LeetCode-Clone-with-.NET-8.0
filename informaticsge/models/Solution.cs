using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using informaticsge.models;

namespace informaticsge.Models
{
    public class Solution
    {
        [Key]
        public int Id { get; set; }
        
        public string Auth_Username { get; set; }
        
        // Foreign key for the user
        public string UserId { get; set; }
        
        public string Status { get; set; }
        
        public string Code { get; set; }
        
        public int Problem_id { get; set; }
        
        public string Problem_name { get; set; }
        
        // Navigation property for the user
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}