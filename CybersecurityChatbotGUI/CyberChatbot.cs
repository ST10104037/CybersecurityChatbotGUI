using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    public class CyberChatbot
    {
        private string userName = "";
        private string userInterest = "";
        private string userSentiment = "";

        private Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>
        {
            { "password", new List<string> { "Use strong, unique passwords.", "Avoid personal details in passwords.", "Enable 2FA if possible." } },
            { "scam", new List<string> { "Beware of unsolicited emails.", "Scammers impersonate trusted brands.", "Report suspicious messages." } },
            { "privacy", new List<string> { "Review social media privacy settings.", "Share less personal info online.", "Use encrypted apps." } },
            { "phishing", new List<string> { "Don't click suspicious email links.", "Check sender's email carefully.", "Phishing uses urgency to trick you." } }
        };

        private Dictionary<string, string> sentimentResponses = new Dictionary<string, string>
        {
            { "worried", "It's normal to be worried. Cyber threats are real, but you can protect yourself." },
            { "curious", "Curiosity is great! Let's learn something new about cybersecurity." },
            { "frustrated", "I understand—cybersecurity can be tricky. I'm here to help." }
        };

        public List<string> ProcessInput(string inputRaw)
        {
            string input = inputRaw.ToLower();
            List<string> responses = new List<string>();

            string sentiment = DetectSentiment(input);
            if (!string.IsNullOrEmpty(sentiment))
            {
                userSentiment = sentiment;
                responses.Add(sentimentResponses[sentiment]);
            }

            string keyword = RecognizeKeyword(input);
            if (!string.IsNullOrEmpty(keyword))
            {
                string response = GetRandomResponse(keyword);
                if (userInterest == keyword)
                    responses.Add($"As someone interested in {keyword}, here's a tip: {response}");
                else
                    responses.Add(response);
            }
            else if (input.Contains("my name is"))
            {
                userName = input.Substring(input.LastIndexOf("is") + 3).Trim();
                responses.Add($"Nice to meet you, {userName}!");
            }
            else if (input.Contains("interested in"))
            {
                userInterest = input.Substring(input.LastIndexOf("in") + 3).Trim();
                responses.Add($"Got it! I'll remember that you're interested in {userInterest}.");
            }
            else
            {
                responses.Add("I'm not sure I understand. Can you try rephrasing?");
            }

            return responses;
        }

        private string DetectSentiment(string input)
        {
            if (input.Contains("worried")) return "worried";
            if (input.Contains("curious")) return "curious";
            if (input.Contains("frustrated")) return "frustrated";
            return "";
        }

        private string RecognizeKeyword(string input)
        {
            foreach (var key in keywordResponses.Keys)
            {
                if (input.Contains(key))
                    return key;
            }
            return "";
        }

        private string GetRandomResponse(string keyword)
        {
            var responses = keywordResponses[keyword];
            Random rnd = new Random();
            return responses[rnd.Next(responses.Count)];
        }
    }
}
