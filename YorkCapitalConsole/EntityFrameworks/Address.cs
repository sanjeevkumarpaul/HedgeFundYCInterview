namespace EntityFrameworks
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class Address
    {
        public int Id { get; set; }

        public int Number { get; set; }

        [Required]
        [StringLength(100)]
        public string Street { get; set; }

        public int? Appartment { get; set; }

        [StringLength(100)]
        public string Society { get; set; }

        [Required]
        [StringLength(75)]
        public string City { get; set; }

        [Required]
        [StringLength(75)]
        public string State { get; set; }

        [Required]
        [StringLength(75)]
        public string Country { get; set; }

        [StringLength(20)]
        public string PrimaryPhone { get; set; }

        [StringLength(20)]
        public string OtherPhone { get; set; }

        [StringLength(150)]
        public string Email { get; set; }
    }
}
