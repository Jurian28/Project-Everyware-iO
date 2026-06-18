using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClassLibrary.DTOs.Sessions
{
    public class SessionReviewDTO
    {
        public int Rating { get; set; }
        public required string Comment { get; set; }
    }
}
