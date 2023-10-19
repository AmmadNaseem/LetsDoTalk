import { Message } from './../models/message';
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrivateChatComponent } from '../private-chat/private-chat.component';

@Injectable({
  providedIn: 'root'
})
export class ChatServiceService {
  myName:string='';
  private chatConnection?:HubConnection;
  onlineUsers:string[]=[];
  messages:Message[]=[];
  privateMessages:Message[]=[];
  privateMessageInitiated=false;

  constructor(private httpClient:HttpClient,private modalService:NgbModal) { }

  registerUser(user:User){
    return this.httpClient.post(`${environment.apiUrl}api/chat/register-user`,user,{responseType:'text'});
  }

  createChatConnection(){
    this.chatConnection= new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}hubs/chat`).withAutomaticReconnect().build();

      this.chatConnection.start().catch(error=>{
        console.log(error);
      });

      //============start user get connection id of user
      this.chatConnection.on('UserConnected',()=>{
        // console.info('Server has called here...');
        this.addUserConnectionId();        
      });

      //============get all online users======
      this.chatConnection.on('OnlineUsers',(onlineUsers)=>{
        this.onlineUsers=[...onlineUsers];
      });

      //===========get new messages=======
      this.chatConnection.on("NewMessage",(newMessage:Message)=>{
        this.messages=[...this.messages,newMessage];
      })

      //===========open private chat
      this.chatConnection.on("OpenPrivateChat",(newMessage:Message)=>{
        this.privateMessages=[...this.privateMessages,newMessage];
        this.privateMessageInitiated=true;
        const modalRef=this.modalService.open(PrivateChatComponent);
        //============passing values to component variables
        modalRef.componentInstance.toUser=newMessage.from;
      })

      //===========privateMessages of private chat
      this.chatConnection.on("NewPrivateMessage",(newMessage:Message)=>{
        this.privateMessages=[...this.privateMessages,newMessage];
      })

      //===========closing private chat
      this.chatConnection.on("ClosePrivateChat",()=>{
        this.privateMessageInitiated=false;
        this.privateMessages=[];
        this.modalService.dismissAll();
      })
  }

  stopChatConnection(){
    this.chatConnection?.stop().catch(error=>console.log(error));
  }

  async addUserConnectionId(){
    //======HERE I AM INVOKING THE API FUNCTION============
    return this.chatConnection?.invoke('AddUserConnectionId',this.myName)
      .catch(error=>console.error(error));       
  }


  async sendMessage(content:string){
    const message:Message={
      from:this.myName,
      content
    }
    return this.chatConnection?.invoke('ReceiveMessage',message)
      .catch(error=>console.error(error)); 
  }

  async sendPrivateMessage(to:string,content:string){
    const message:Message={
      from:this.myName,
      to,
      content
    }
    if (!this.privateMessageInitiated) {
      this.privateMessageInitiated=true;
      return this.chatConnection?.invoke('CreatePrivateChat',message)
      .then(()=>{
        this.privateMessages=[...this.privateMessages,message];
      })
      .catch(error=>console.error(error)); 
    }else{
      return this.chatConnection?.invoke('ReceivedPrivateMessage',message)
      .catch(error=>console.error(error)); 
    }

  }

  async closePrivateChatMessage(otherUser:string){
    //==============invoke is used for calling backend function
    return this.chatConnection?.invoke('RemovePrivateChat',this.myName,otherUser)
    .catch(error=>console.error(error))
  }


}
