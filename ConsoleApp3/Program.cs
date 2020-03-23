using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SQLConnectionPooling
{
    class Program
    {
        static PerformanceCounter pooledConn;
        static Process currentProcess;
        static int numActiveConns;

        static void Main(string[] args)
        {
            currentProcess = Process.GetCurrentProcess();
            pooledConn = new PerformanceCounter(categoryName: ".NET Data Provider for SqlServer", counterName: "NumberOfActiveConnections", instanceName: currentProcess.ProcessName.ToLower() + "[" + currentProcess.Id + "]");

            using (SqlConnection cnn = new SqlConnection("Server=tcp:localhost,1433;Initial Catalog=test;Integrated Security=SSPI;Connection Lifetime=0"))
            {
                cnn.StateChange += Cnn_StateChange;
                cnn.Open();

                numActiveConns = (int)pooledConn.NextValue();
                Console.WriteLine("\n[{0}] -- Number of Active Connections: {1} -- \n", DateTime.Now.ToUniversalTime(),numActiveConns);
            }

            numActiveConns = (int)pooledConn.NextValue();
            Console.WriteLine("\n[{0}] -- Number of Active Connections: {1} -- \n", DateTime.Now.ToUniversalTime(),numActiveConns);

            Console.ReadLine();
        }

        private static void Cnn_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            Console.WriteLine("[{0}] -- CurrentState: {0} -- OriginalState: {1} \n", DateTime.Now.ToUniversalTime(), e.CurrentState, e.OriginalState);
        }
    }
}
