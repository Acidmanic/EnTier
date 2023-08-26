using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Reflection.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Example.Meadow.Models
{
    public class Post
    {
        [UniqueMember] [AutoValuedMember] public long Id { get; set; }

        [FilterField] public string Title { get; set; }

        [FilterField] public string Content { get; set; }
    }
}