using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookNest.Data.Entities
{
    public class Role : IdentityRole<long>
    {
        //IdentityRole already gives id and name property
    }
}
