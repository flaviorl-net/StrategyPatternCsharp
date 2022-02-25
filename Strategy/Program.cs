using System;
using System.Linq;
using System.Collections.Generic;

namespace Strategy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Escolha a operação: (sum, sub, mult, div): ");
            string operacao = Console.ReadLine();

            Console.Write("Digite o primeiro número: ");
            int.TryParse(Console.ReadLine(), out int numero1);
            Console.Write("Digite o segundo número: ");
            int.TryParse(Console.ReadLine(), out int numero2);

            var calculadoraStrategy = new CalculadoraStrategy(operacao);
            int resultado = calculadoraStrategy.Calcular(numero1, numero2);

            Console.WriteLine("Resultado: {0}", resultado);
        }
    }

    public class CalculadoraStrategy
    {
        public enum Operacao
        {
            None,
            Sum,
            Sub,
            Mult,
            Div
        }

        private readonly ContextStrategy<int, StrcNumeros, Operacao> _contextStrategy = new ContextStrategy<int, StrcNumeros, Operacao>();

        public CalculadoraStrategy(string operacao)
        {
            Operacao op = Operacao.None;

            try
            {
                op = (Operacao)Enum.Parse(typeof(Operacao), operacao, true);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Operação inválida!");
                return;
            }
            
            _contextStrategy.AddList(Operacao.Sum, new Sum());
            _contextStrategy.AddList(Operacao.Sub, new Sub());
            _contextStrategy.AddList(Operacao.Mult, new Mult());
            _contextStrategy.AddList(Operacao.Div, new Div());

            SelecionarCalculo(op);
        }

        private void SelecionarCalculo(Operacao operacao)
        {
            _contextStrategy.SetStrategy(operacao);
        }

        public int Calcular(int numero1, int numero2)
        {
            return _contextStrategy.Execute(new StrcNumeros() { Numero1 = numero1, Numero2 = numero2 });
        }
    }

    public class ContextStrategy<TResponse, TRequest, TSearch>
    {
        private IStrategy<TResponse, TRequest> _strategy;
        private readonly Dictionary<TSearch, IStrategy<TResponse, TRequest>> _dicStrategy = new Dictionary<TSearch, IStrategy<TResponse, TRequest>>();

        public void AddList(TSearch op, IStrategy<TResponse, TRequest> str)
        {
            _dicStrategy.Add(op, str);
        }

        public void SetStrategy(TSearch op)
        {
            _strategy = _dicStrategy.FirstOrDefault(x => x.Key.GetHashCode() == op.GetHashCode()).Value;
        }

        public TResponse Execute(TRequest request)
        {
            if (_strategy == null)
                return default(TResponse);
            
            var stratg = _strategy.GetType();
            var notify = stratg.GetMethod("Execute");
            var instance = Activator.CreateInstance(_strategy.GetType());
            
            return (TResponse)notify.Invoke(instance, new object[] { request });
        }
    }

    public interface IStrategy<out TResponse, in TRequest>
    {
        TResponse Execute(TRequest request);
    }

    public struct StrcNumeros
    {
        public int Numero1 { get; set; }

        public int Numero2 { get; set; }
    }

    /// <summary>
    /// ConcreteStrategy
    /// </summary>
    public class Sum : IStrategy<int, StrcNumeros>
    {
        public int Execute(StrcNumeros numeros)
        {
            return numeros.Numero1 + numeros.Numero2;
        }
    }

    /// <summary>
    /// ConcreteStrategy
    /// </summary>
    public class Sub : IStrategy<int, StrcNumeros>
    {
        public int Execute(StrcNumeros numeros)
        {
            return numeros.Numero1 - numeros.Numero2;
        }
    }

    /// <summary>
    /// ConcreteStrategy
    /// </summary>
    public class Mult : IStrategy<int, StrcNumeros>
    {
        public int Execute(StrcNumeros numeros)
        {
            return numeros.Numero1 * numeros.Numero2;
        }
    }

    /// <summary>
    /// ConcreteStrategy
    /// </summary>
    public class Div : IStrategy<int, StrcNumeros>
    {
        public int Execute(StrcNumeros numeros)
        {
            if (numeros.Numero2 == 0)
                throw new DivideByZeroException();

            return numeros.Numero1 / numeros.Numero2;
        }
    }

}
