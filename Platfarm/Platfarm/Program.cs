namespace Platfarm
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (Platfarm game = new Platfarm())
            {
                game.Run();
            }
        }
    }
#endif
}

