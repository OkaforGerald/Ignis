using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Image
    {
        public Guid Id { get; set; }

        public string? FileName { get; set; }

        public long Size { get; set; }

        public DateTime AddedAt { get; set; }

        public DateTime LastModified {  get; set; }

        public string? ThumbUrl { get; set; }

        public string? HiResUrl { get; set; }

        public ExifMetadata ExifMetadata { get; set; }

        public User Owner { get; set; }
        public string OwnerId { get; set; }
    }

    [ComplexType]
    public class ExifMetadata
    {
        public string? Manufacturer { get; set; }

        public string? Model { get; set; }

        public DateTime DateTaken { get; set; }

        public float FocalLength { get; set; }

        public float GPSLongitude { get; set; }

        public float GPSLatitude { get; set; }

        public float GPSAltitude { get; set; }

        public int ISO {  get; set; }
    }
}
