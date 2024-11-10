﻿using System.ComponentModel.DataAnnotations;

namespace Server.Models.DTOs
{
    public class GetAccountDetails
    {
        public long AccountNumber { get; set; }

        public long? CRN { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}