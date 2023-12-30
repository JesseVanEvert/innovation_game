namespace UserTeamOrg.Model.Entity
{
    public class Team
    {
        public Guid TeamID { get; set; }
        public Guid OrganizationID { get; set; }
        public Organization Organization { get; set; }

        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual ICollection<UserTeam> UserTeams { get; set; }
    }
}
