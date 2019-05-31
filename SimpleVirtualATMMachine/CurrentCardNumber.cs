

namespace SimpleVirtualATMMachine
{
    public interface ICurrentCardNumber
    {
        string Number { get; set; }
    }

    public class CurrentCardNumber : ICurrentCardNumber
    {
        public string Number { get; set; } 
    }
}
