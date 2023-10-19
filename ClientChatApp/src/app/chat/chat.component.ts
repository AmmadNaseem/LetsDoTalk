import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { ChatServiceService } from '../Services/chat-service.service';
import { PrivateChatComponent } from '../private-chat/private-chat.component';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {
  @Output() closeChatEmitter=new EventEmitter();

  constructor(public chatService:ChatServiceService,private modalService:NgbModal) { }

  ngOnInit(): void {
    this.chatService.createChatConnection();
  }
  ngOnDestroy(){
    this.chatService.stopChatConnection();
  }

  backToHome(){
    this.closeChatEmitter.emit();
  }

  sendMessage(content:string){
    this.chatService.sendMessage(content);
  }

  openPrivateChat(toUser:string){
    const modalRef=this.modalService.open(PrivateChatComponent);
    modalRef.componentInstance.toUser=toUser;

  }

}
