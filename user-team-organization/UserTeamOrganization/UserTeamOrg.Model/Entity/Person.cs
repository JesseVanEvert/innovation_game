using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace UserTeamOrg.Model.Entity
{
    public class Person
    {
        public Guid PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password
        {
            get { return PasswordStored; }
            set { PasswordStored = Encrypt(value); }
        }
        private string? PasswordStored { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Admin Admin { get; set; }
        public User User { get; set; }

        private string Encrypt(string data)
        {
            data += Environment.GetEnvironmentVariable("PasswordSalt");
            using (var hashAlgorithm = SHA512.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(data);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }
    }
}

