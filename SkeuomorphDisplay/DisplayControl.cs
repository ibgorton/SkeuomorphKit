namespace SkeuomorphDisplay
{
    public interface IDisplayControl
    {
        double IncrementFactor { get; set; }

        void BlankModule();

        void SetChar(char character);

        void SetColorBrightness();
    }
}
