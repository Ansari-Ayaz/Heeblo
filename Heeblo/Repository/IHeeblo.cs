using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IHeeblo
    {
        //string Plagiarism(string content);
        //string AiDetect(string content);
        //string Grammer(string content);
        Task GetScores(string content, int id);
        bool SendEmail(string emaiId, string subject, string body);


    }
}
