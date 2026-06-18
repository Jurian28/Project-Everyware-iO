using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClassLibrary.DTOs.Sessions
{
    public class SessionReviewsResponseDTO
    {
        public bool CanReview { get; set; }
        public List<SessionReviewDTO> Reviews { get; set; } = new();
    }
}
