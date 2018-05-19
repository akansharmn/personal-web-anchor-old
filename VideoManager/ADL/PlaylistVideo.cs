using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADL
{
    [Table("PlaylistVideos")]
    public partial class PlaylistVideo
    {
        public int Id { get; set; }

        [Key, Column(Order = 1)]
        public int PlaylistId { get; set; }

        [Key, Column(Order = 2)]
        public int VideoId { get; set; }
    }
}