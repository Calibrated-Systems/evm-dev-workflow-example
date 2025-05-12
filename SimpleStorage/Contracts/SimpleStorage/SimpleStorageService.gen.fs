namespace Contracts.SimpleStorage

open System
open System.Threading.Tasks
open System.Collections.Generic
open System.Numerics
open Nethereum.Hex.HexTypes
open Nethereum.ABI.FunctionEncoding.Attributes
open Nethereum.Web3
open Nethereum.RPC.Eth.DTOs
open Nethereum.Contracts.CQS
open Nethereum.Contracts.ContractHandlers
open Nethereum.Contracts
open System.Threading
open Contracts.SimpleStorage.ContractDefinition


    type SimpleStorageService (web3: Web3, contractAddress: string) =
    
        member val Web3 = web3 with get
        member val ContractHandler = web3.Eth.GetContractHandler(contractAddress) with get
    
        static member DeployContractAndWaitForReceiptAsync(web3: Web3, simpleStorageDeployment: SimpleStorageDeployment, ?cancellationTokenSource : CancellationTokenSource): Task<TransactionReceipt> = 
            let cancellationTokenSourceVal = defaultArg cancellationTokenSource null
            web3.Eth.GetContractDeploymentHandler<SimpleStorageDeployment>().SendRequestAndWaitForReceiptAsync(simpleStorageDeployment, cancellationTokenSourceVal)
        
        static member DeployContractAsync(web3: Web3, simpleStorageDeployment: SimpleStorageDeployment): Task<string> =
            web3.Eth.GetContractDeploymentHandler<SimpleStorageDeployment>().SendRequestAsync(simpleStorageDeployment)
        
        static member DeployContractAndGetServiceAsync(web3: Web3, simpleStorageDeployment: SimpleStorageDeployment, ?cancellationTokenSource : CancellationTokenSource) = async {
            let cancellationTokenSourceVal = defaultArg cancellationTokenSource null
            let! receipt = SimpleStorageService.DeployContractAndWaitForReceiptAsync(web3, simpleStorageDeployment, cancellationTokenSourceVal) |> Async.AwaitTask
            return new SimpleStorageService(web3, receipt.ContractAddress);
            }
    
        member this.GetQueryAsync(getFunction: GetFunction, ?blockParameter: BlockParameter): Task<BigInteger> =
            let blockParameterVal = defaultArg blockParameter null
            this.ContractHandler.QueryAsync<GetFunction, BigInteger>(getFunction, blockParameterVal)
            
        member this.SetRequestAsync(setFunction: SetFunction): Task<string> =
            this.ContractHandler.SendRequestAsync(setFunction);
        
        member this.SetRequestAndWaitForReceiptAsync(setFunction: SetFunction, ?cancellationTokenSource : CancellationTokenSource): Task<TransactionReceipt> =
            let cancellationTokenSourceVal = defaultArg cancellationTokenSource null
            this.ContractHandler.SendRequestAndWaitForReceiptAsync(setFunction, cancellationTokenSourceVal);
        
    

