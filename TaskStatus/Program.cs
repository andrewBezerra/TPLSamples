using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskStatus
{
    /*
    ESTE PROJETO TEM A FINALIDADE DE SIMULAR OS DIFERENTES STATUS QUE UMA TASK PODE RETORNAR.
    */
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();



            Console.Clear();
            Console.WriteLine("Aplicação que captura os diferentes TaskStatus possíveis.");
            Next();

            Console.WriteLine("\n###########################################################\n");
            System.Console.WriteLine("T1 Created = UMA TAREFA CONTRUIDA MAS N INICIADA");
            var t1 = new Task(() => longProcess(0, cts.Token), cts.Token);
            Console.WriteLine($"T1 STATUS: {t1.Status.ToString()}");
            Console.WriteLine("\n###########################################################\n");

            Next();

            Console.WriteLine("\n###########################################################\n");
            System.Console.WriteLine("T2 WaitingToRun: UMA TAREFA FOI AGENDADA MAS N INICIADA AINDA");
            var t2 = Task.Run(() => longProcess(0, cts.Token), cts.Token);
            Console.WriteLine($"T2 STATUS: {t2.Status.ToString()}");
            Console.WriteLine("\n###########################################################\n");
            Next();

            Console.WriteLine("\n###########################################################\n");
            System.Console.WriteLine("T3 RanToCompletion:UMA TAREFA QUE FOI INICIADA E FOI CONCLUIDA");
            var t3 = Task.Run(() => longProcess(10, cts.Token), cts.Token)
                            .ContinueWith(t =>
                            {
                                Console.WriteLine($"T3 STATUS: {t.Status.ToString()}");
                                Console.WriteLine("\n###########################################################\n");
                            });

            // Console.WriteLine($"T3 WaitingForActivation -> Enquando a T3 não completa seu STATUS: {t3.Status}");
            Thread.Sleep(100);
            Next();

            Console.WriteLine("\n###########################################################\n");
            System.Console.WriteLine("T4 Canceled :UMA TAREFA QUE FOI INICIADA E LOGO APÓS CANCELADA");
            cts.CancelAfter(1);
            var t4 = Task.Run(() => longProcess(10, cts.Token), cts.Token)
                        .ContinueWith(t =>
                        {
                            Console.WriteLine($"T4 STATUS: {t.Status.ToString()}");
                            Console.WriteLine("\n###########################################################\n");
                        }); ;
            Thread.Sleep(100);
            Next();

            Console.WriteLine("\n###########################################################\n");
            System.Console.WriteLine("T5 Falted :UMA TAREFA QUE FOI INICIADA E LOGO APÓS CANCELADA.\n POREM NÃO COMO REPASSADO O SINAL DE CANCELAMENTO PARA A TAREFA PRINCIPAL \n A TAREFA PRINCIPAL FOI CONCLUIDA COM EXCEÇÕES");
            cts.CancelAfter(1);
            var t5 = Task.Run(() => longProcess(10, cts.Token))
                        .ContinueWith(t =>
                        {
                            Console.WriteLine($"T5 STATUS: {t.Status.ToString()}");
                            Console.WriteLine("\n###########################################################\n");
                        }); ;
            Thread.Sleep(100);
            Next();

            Console.WriteLine("\n###########################################################\n");
            Console.WriteLine("T6 UMA TAREFA QUE É CONSTRUIDA E AGENDADA, ENTRA EM EXECUÇÃO E É COMPLETADA");
            var T6 = Task.Factory.StartNew(() => TaskRunning(10), TaskCreationOptions.RunContinuationsAsynchronously);
            Console.WriteLine($"T6 STATUS: {T6.Status}");
            Thread.Sleep(2);
            Console.WriteLine($"T6 STATUS: {T6.Status}");
            Thread.Sleep(100);
            Console.WriteLine($"T6 STATUS: {T6.Status}");
            Console.WriteLine("\n###########################################################\n");
            Thread.Sleep(100);
            Next();

            Console.WriteLine("\n###########################################################\n");
            Console.WriteLine("T7 UMA TAREFA QUE PRECISA ESPERAR UMA TAREFA FILHA COMPLETAR");
            var T7 = Task.Factory.StartNew(() => TaskRunningWithChildTask(10));
            Thread.Sleep(10);
            Console.WriteLine($"T7 STATUS: {T7.Status}");
            Console.WriteLine("\n###########################################################\n");
            Thread.Sleep(100);
            Next();


            Console.WriteLine("\n###########################################################\n");
            Console.WriteLine("T8 UMA TAREFA QUE ESTA AGUARDANDO QUE OPERAÇOES DEPENDENTES CONCLUAM");
            var T8 = Task.Run(() => TaskRunningWithChildTask(100));
            Thread.Sleep(90);
            Console.WriteLine($"T8 STATUS: {T8.Status}");
            Thread.Sleep(30);
            Console.WriteLine($"T8 STATUS: {T8.Status}");
            Console.WriteLine("\n###########################################################\n");
            Thread.Sleep(100);
            Next();




        }
        public static void Next()
        {
            Console.WriteLine("APERTE UMA TECLA PARA IR AO PROXIMO EXEMPLO");
            Console.ReadKey();
            Console.Clear();
        }
        public static void unsafeLongProcess(int time, CancellationToken token)
        {

            for (int i = 0; i < 1000; i++)
            {

                if (token.IsCancellationRequested)
                    break;


            }
            //return Task.CompletedTask;
        }
        public static int longProcess(int time, CancellationToken token)
        {
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                count++;
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                if (time > 0)
                    Thread.Sleep(time);


            }

            return count;
        }

        public static async Task TaskRunning(int times)
        {
            for (int i = 0; i < times; i++)
            {
                Thread.Sleep(1);
            }
            await Task.Delay(1);


        }

        public static async Task TaskRunningWithChildTask(int times)
        {
            var child = Task.Factory.StartNew(() => TaskRunning(100), TaskCreationOptions.AttachedToParent).ConfigureAwait(true);

            await Task.Delay(100);
        }
    }
}
