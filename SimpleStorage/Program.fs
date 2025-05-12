open System
open System.IO
open System.Threading.Tasks
open System.Numerics
open Nethereum.Web3
open Nethereum.Web3.Accounts
open SimpleStorage.WalletManager
open Contracts.SimpleStorage
open Contracts.SimpleStorage.ContractDefinition

// AVAX Fuji Testnet configuration
let avaxFujiRpcUrl = "https://api.avax-test.network/ext/bc/C/rpc"
let avaxFujiChainId = 43113

// Path to keystore directory
let keystorePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Wallet", "keystore")

/// <summary>
/// Deploys the SimpleStorage contract and returns the contract address
/// </summary>
let deployContract (web3: Web3) =
    // Create a new SimpleStorageDeployment
    let deployment = new SimpleStorageDeployment()
    
    // Deploy the contract and wait for receipt
    let receipt = SimpleStorageService.DeployContractAndWaitForReceiptAsync(web3, deployment).Result
    
    // Return the contract address
    receipt.ContractAddress

/// <summary>
/// Calls the set function of the SimpleStorage contract
/// </summary>
let setStoredValue (web3: Web3) (contractAddress: string) (value: int) =
    // Create a new instance of the SimpleStorageService with the contract address
    let service = new SimpleStorageService(web3, contractAddress)
    
    // Create a new SetFunction with the value
    let setFunction = new SetFunction()
    setFunction.X <- BigInteger(value)
    
    // Call the SetRequestAndWaitForReceiptAsync method
    service.SetRequestAndWaitForReceiptAsync(setFunction)

/// <summary>
/// Calls the get function of the SimpleStorage contract
/// </summary>
let getStoredValue (web3: Web3) (contractAddress: string) =
    // Create a new instance of the SimpleStorageService with the contract address
    let service = new SimpleStorageService(web3, contractAddress)
    
    // Create a new GetFunction
    let getFunction = new GetFunction()
    
    // Call the GetQueryAsync method
    let task = service.GetQueryAsync(getFunction)
    
    // Convert the result to an int
    Task.FromResult(int (task.Result))

/// <summary>
/// Main function
/// </summary>
[<EntryPoint>]
let main argv =
    try
        printfn "F# Nethereum SimpleStorage Example"
        printfn "-----------------------------------"
        
        // Get or create wallet
        let account = getOrCreateWallet keystorePath
        printfn $"Using account: {account.Address}"
        
        // Create web3 instance with AVAX Fuji Testnet
        let web3 = new Web3(account, avaxFujiRpcUrl)
        
        // Check connection
        let chainId = web3.Eth.ChainId.SendRequestAsync().Result
        printfn $"Connected to chain ID: {chainId}"
        if chainId.Value <> BigInteger.Parse(avaxFujiChainId.ToString()) then
            printfn "Warning: Not connected to AVAX Fuji Testnet"
        
        // Deploy contract
        printfn "Deploying contract..."
        try
            (*let contractAddress = deployContract(web3)
            printfn $"Contract deployed at: {contractAddress}"*)
            let contractAddress = "0xc29dd4d8423cc1ae4f7fa1a5f4ae585eb0747f27"
            
            // Get value
            printfn "Getting value..."
            let value = getStoredValue web3 contractAddress |> Async.AwaitTask |> Async.RunSynchronously
            printfn $"Stored value: {value}"
            
            // Set value
            (*printfn "Setting value to 48..."
            setStoredValue web3 contractAddress 48 |> Async.AwaitTask |> Async.RunSynchronously |> ignore*)
            
            printfn "Done!"
            0
        with
        | ex ->
            printfn $"Error during contract operations: {ex.Message}"
            printfn "This is expected if there are issues with the contract or network."
            1
    with
    | ex ->
        printfn $"Error: {ex.Message}"
        1
