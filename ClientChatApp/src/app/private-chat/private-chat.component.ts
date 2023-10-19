import { ChatServiceService } from './../Services/chat-service.service';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-private-chat',
  templateUrl: './private-chat.component.html',
  styleUrls: ['./private-chat.component.scss']
})
export class PrivateChatComponent implements OnInit,OnDestroy {
  @Input() toUser='';

  constructor(public activeModel:NgbActiveModal, public chatService:ChatServiceService) { }
  ngOnDestroy(): void {
    this.chatService.closePrivateChatMessage(this.toUser);
  }

  ngOnInit(): void {
  }

  sendMessage(content:string){
    this.chatService.sendPrivateMessage(this.toUser,content);
  }
}
