using EnTier.Regulation;
using ExampleRegulation.Contracts;
using ExampleRegulation.Models;

namespace ExampleRegulation.Regulators
{
    public class PostRegulator : DataAccessRegulatorBase<Post, Post>, IPostRegulator
    {

        public override RegulationResult<Post> RegulateIncoming(Post model)
        {
            if (model == null || string.IsNullOrEmpty(model.Title))
            {
                return Reject<Post>();
            }
            
            if (string.IsNullOrEmpty(model.Content))
            {
                model.Content = model.Title + ": Empty content would not be saved!";

                return Suspect(model);
            }
            
            return Accept(model);
        }

        public override RegulationResult<Post> RegulateOutgoing(Post model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                model.Title = "Big Empty Title!";

                return Suspect(model);
            }

            if (model.Title.ToLower().Contains("secrete"))
            {
                model.Title = "[Secrete Post]";
                
                model.Content = "[*******************]";
                
            }
            return Accept(model);
        }
    }
}