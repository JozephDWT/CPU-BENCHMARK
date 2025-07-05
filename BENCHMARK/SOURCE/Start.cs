using BENCHMARK.SOURCE.CPU;
using BENCHMARK.SOURCE.METHOD;

internal static class Start
{
    private static void Main()
    {
        int streams = ThreadCheck.Logic();          // логические потоки
        int physical = PhysicalCheck.Logic();       // физические ядра

        Console.WriteLine($"ПРОЦЕССОР: {CPU.Name}, ЯДЕР: {physical}, ПОТОКОВ: {streams}");
        Console.WriteLine("Бенчмарк запущен, ожидайте результатов\n");

        // Тяжёлая математика над double
        TimeSpan t1 = HeavyMath.Run();
        Console.WriteLine($"Тяжёлая математика над double: {t1.TotalSeconds:F2} сек");

        // FMA
        TimeSpan t2 = CpuFma.Run();
        Console.WriteLine($"Регистровый FMA-тест: {t2.TotalSeconds:F2} сек");

        // подсчёт до 100 млрд
        TimeSpan t3 = Billion.Run();
        Console.WriteLine($"Подсчёт от 0 до 100 млрд: {t3.TotalSeconds:F2} сек");

        // BBP-бенчмарк: на каждом ядре вычисляется first-N членов ряда
        TimeSpan t4 = BBP.Run(); Console.WriteLine($"BBP-тест π: {BBP.IterPerCore:N0} членов ряда на поток — выполнено за {t4.TotalSeconds:F2} сек");

        TimeSpan total = t1 + t2 + t3;
        Console.WriteLine($"\nВсего тесты заняли: {total.TotalSeconds:F2} секунд\n");
        Console.WriteLine("Нажмите Enter для выхода");
        Console.ReadLine();
    }
}