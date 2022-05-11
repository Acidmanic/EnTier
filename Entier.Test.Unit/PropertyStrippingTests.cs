using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EnTier.Repositories;
using EnTier.Repositories.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Xunit;

namespace Entier.Test.Unit
{
    public class PropertyStrippingTests
    {
        private class Property
        {
            public string Name { get; set; }
        }

        private class KeepingProperty
        {
            public long Id { get; set; }
        }

        private class Model
        {
            public long Id { get; set; }

            public Property Property { get; set; }

            public KeepingProperty KeepingProperty { get; set; }
        }

        private class DummyRepo : CrudRepositoryBase<Model, long>
        {
            public override IEnumerable<Model> All()
            {
                throw new NotImplementedException();
            }

            protected override Model Insert(Model value)
            {
                return value;
            }


            public override Model GetById(long id)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<Model> Find(Expression<Func<Model, bool>> predicate)
            {
                throw new NotImplementedException();
            }

            public override bool Remove(Model value)
            {
                throw new NotImplementedException();
            }

            public override bool Remove(long id)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void RepositoryShouldStrippAllSubperopertiesByDefault()
        {
            var repo = new DummyRepo();

            var model = new Model
            {
                Id = 100,
                Property = new Property
                {
                    Name = "Pashmak"
                },
                KeepingProperty = new KeepingProperty
                {
                    Id = 200
                }
            };

            Model stripped = repo.Add(model);

            Assert.NotNull(stripped);

            Assert.Equal(model.Id, stripped.Id);

            Assert.Null(stripped.Property);

            Assert.Null(stripped.KeepingProperty);
        }

        [Fact]
        [KeepProperty(typeof(KeepingProperty))]
        public void RepositoryShouldStripPropertyAndKeepKeepingProperty()
        {
            var repo = new DummyRepo();

            var model = new Model
            {
                Id = 100,
                Property = new Property
                {
                    Name = "Pashmak"
                },
                KeepingProperty = new KeepingProperty
                {
                    Id = 200
                }
            };

            Model stripped = repo.Add(model);

            Assert.NotNull(stripped);

            Assert.Equal(model.Id, stripped.Id);

            Assert.Null(stripped.Property);

            Assert.NotNull(stripped.KeepingProperty);

            Assert.Equal(model.KeepingProperty.Id, stripped.KeepingProperty.Id);
        }
        
        [Fact]
        [KeepAllProperties()]
        [StripAllProperties()]
        public void RepositoryShouldStripAllSubProperties()
        {
            var repo = new DummyRepo();

            var model = new Model
            {
                Id = 100,
                Property = new Property
                {
                    Name = "Pashmak"
                },
                KeepingProperty = new KeepingProperty
                {
                    Id = 200
                }
            };

            Model stripped = repo.Add(model);

            Assert.NotNull(stripped);

            Assert.Equal(model.Id, stripped.Id);

            Assert.Null(stripped.Property);

            Assert.Null(stripped.KeepingProperty);
        }
        
        [Fact]
        [StripAllProperties()]
        [KeepAllProperties()]
        public void RepositoryShouldKeepAllSubProperties()
        {
            var repo = new DummyRepo();

            var model = new Model
            {
                Id = 100,
                Property = new Property
                {
                    Name = "Pashmak"
                },
                KeepingProperty = new KeepingProperty
                {
                    Id = 200
                }
            };

            Model stripped = repo.Add(model);

            Assert.NotNull(stripped);

            Assert.Equal(model.Id, stripped.Id);

            Assert.NotNull(stripped.Property);
            
            Assert.Equal(model.Property.Name,stripped.Property.Name);

            Assert.NotNull(stripped.KeepingProperty);
            
            Assert.Equal(model.Property.Name,stripped.Property.Name);
        }
    }
}