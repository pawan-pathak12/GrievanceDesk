using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class AddNoteRequest
    {
        [Required, StringLength(2000, MinimumLength = 3)]
        public string NoteText { get; set; } = string.Empty;
    }
}
