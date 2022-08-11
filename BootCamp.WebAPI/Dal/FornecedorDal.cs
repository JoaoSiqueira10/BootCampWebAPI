using BootCamp.WebAPI.Dal.Repositories;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace BootCamp.WebAPI.Dal
{
    public class FornecedorDal : IFornecedor
    {
        string projectId;
        FirestoreDb fireStoreDb;
        public FornecedorDal()
        {
            /*Caminho do arquivo baixado do firebase ou gcloud, colocar na raiz do projeto*/
            string arquivoApiKey = @"NomeArquivo.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", arquivoApiKey);
            projectId = "NomeProjeto";
            fireStoreDb = FirestoreDb.Create(projectId);
        }
        public async Task<List<Model.Fornecedor>> GetForncedores()
        {
            try
            {
                Query fornecedoroQuery = fireStoreDb.Collection("fornecedor");
                QuerySnapshot inscricaoQuerySnaphot = await fornecedoroQuery.GetSnapshotAsync();
                List<Model.Fornecedor> listaFornecedor = new List<Model.Fornecedor>();
                foreach (DocumentSnapshot documentSnapshot in inscricaoQuerySnaphot.Documents)
                {
                    if (documentSnapshot.Exists)
                    {
                        Dictionary<string, object> city = documentSnapshot.ToDictionary();
                        string json = JsonConvert.SerializeObject(city);
                        Model.Fornecedor novoFornecedor = JsonConvert.DeserializeObject<Model.Fornecedor>(json);
                        novoFornecedor.Id = documentSnapshot.Id;
                    listaFornecedor.Add(novoFornecedor);
                    }
                }
                List<Model.Fornecedor> listaFornecedorOrdenada = listaFornecedor.OrderBy(x => x.Nome).ToList();
                return listaFornecedorOrdenada;
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                throw;
            }
        }
        public async Task<Model.Fornecedor> GetFornecedor(string id)
        {
            try
            {
                DocumentReference docRef = fireStoreDb.Collection("fornecedor").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Model.Fornecedor fornecedor = snapshot.ConvertTo<Model.Fornecedor>();
                    fornecedor.Id = snapshot.Id;
                    return fornecedor;
                }
                else
                {
                    return new Model.Fornecedor();
                }
            }
            catch
            {
                throw;
            }
        }
        public string AddFornecedor(Model.Fornecedor fornecedor)
        {
            try
            {
                CollectionReference colRef = fireStoreDb.Collection("fornecedor");
                var id = colRef.AddAsync(fornecedor).Result.Id;
                var shardRef = colRef.Document(id.ToString());
                shardRef.UpdateAsync("Id", id);
                return id;
            }
            catch
            {
                return "Error";
            }
        }
        public async void UpdateFornecedor(Model.Fornecedor fornecedor)
        {
            try
            {
                DocumentReference fornecedorRef = fireStoreDb.Collection("fornecedor").Document(fornecedor.Id);
                await fornecedorRef.SetAsync(fornecedor, SetOptions.Overwrite);
            }
            catch
            {
                throw;
            }
        }
        public async void DeleteFornecedor(string id)
        {
            try
            {
                DocumentReference fornecedorRef = fireStoreDb.Collection("fornecedor").Document(id);
                await fornecedorRef.DeleteAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
