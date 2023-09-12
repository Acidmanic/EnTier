using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using EnTier;
using EnTier.Mapper;
using EnTier.Models;
using EnTier.Regulation;
using EnTier.Services;
using EnTier.UnitOfWork;
using ServiceManipulationExample.Models;

namespace ServiceManipulationExample.Services
{
    public class MyPostService : CrudService<Post, Post, long, long>
    {
        public override Chunk<Post> ReadSequence(int offset, int size, string searchId, FilterQuery filterQuery,
            string searchTerm,
            OrderTerm[] orderTerms,
            bool readFullTree = false)
        {
            var originalChunk =
                base.ReadSequence(offset, size, searchId, filterQuery, searchTerm, orderTerms, readFullTree);

            var manipulatedChunk = new Chunk<Post>
            {
                Items = originalChunk.Items.Select(Manipulate),
                Offset = originalChunk.Offset,
                Size = originalChunk.Size,
                TotalCount = originalChunk.TotalCount
            };

            return manipulatedChunk;
        }

        public override Post ReadById(long id, bool readFullTree = false)
        {
            return Manipulate(base.ReadById(id, readFullTree));
        }

        private Post Manipulate(Post p)
        {
            if (p == null)
            {
                return new Post
                {
                    Title = "It Was A Null!",
                    Content = "This post does not exist! It was a null object i took its place!"
                };
            }

            p.Content = "[Manipulated in MYYY Service!!!] " + p.Content;

            return p;
        }

        public MyPostService(EnTierEssence essence) : base(essence)
        {
        }
    }
}