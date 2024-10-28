using IronMan.Data.AggregateRoots;

namespace IronMan.Data.Entities
{
    using Microsoft.EntityFrameworkCore;

    [Owned]
    public class RefreshToken :BasicAggregateRoot<int>
    {   
        public int UserId { get; set; }

        public Account User { get; set; }


        public string Token { get; set; }

        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }

        public string CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? ReasonRevoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
