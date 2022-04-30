using EnTier.Regulation;
using ExampleRegulation.Contracts;
using ExampleRegulation.Models;

namespace ExampleRegulation.Regulators
{
    public class PostRegulator : DataAccessRegulatorBase<Post, Post>, IPostRegulator
    {
        public override RegulationResult<Post, Post> Regulate(Post model)
        {
            if (model == null || string.IsNullOrEmpty(model.Title))
            {
                return Reject(model);
            }

            if (string.IsNullOrEmpty(model.Content))
            {
                model.Content = model.Title;

                return ReportSuspicious(model, model);
            }

            return Accept(model,model);
        }
    }
}