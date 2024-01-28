using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis.courseWebApp.Backend.Models
{
    public class RnnModelService
    {
        private readonly RnnModel _rnnModel;

        public RnnModelService(RnnModel rnnModel)
        {
            _rnnModel = rnnModel;
        }

        public string PreprocessUserInput(string userInput)
        {
            // Convert to lowercase and remove special characters
            string cleanedInput = new string(userInput.ToLower().Where(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c)).ToArray());
            return cleanedInput;
        }

        public async Task<List<string>> Predict(string username)
        {
            try
            {
                // Implement logic to interact with your RNN model for predictions
                var predictions = await _rnnModel.GetPredictions(username);
                return predictions;
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                Console.Error.WriteLine($"Error predicting: {ex.Message}");
                return new List<string>();
            }
        }

        public List<string> ProcessPredictions(List<string> predictions)
        {
            //  Filter or format the predictions
            return predictions.Where(prediction => !string.IsNullOrWhiteSpace(prediction)).ToList();
        }
    }
}
