export class Order {
    id: number;
    title: string;
    quantity: number;
    orderStatus: string;
    dispatchDate: any;
    dispatchOccurence: any;
    orderType: string;

    constructor(response: any) {
        this.id = response.id; 
        this.title = response.title;
        this.dispatchDate = response.dispatchedDate;
        this.dispatchOccurence = response.dispatchOccurence;
        this.quantity = response.quantity;
        this.orderStatus = response.orderStatus;
        this.orderType = response.orderType; 
    }
}

export class Client {
    id: number;
    name: string;
    email: string;
    ordersClient?: Order[];

    constructor(response: any) {
        this.id = response.id; 
        this.name = response.name; 
        this.email = response.email;
        this.ordersClient = response.ordersClient; 
    }
}