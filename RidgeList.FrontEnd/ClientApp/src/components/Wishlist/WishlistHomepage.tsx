import React, {ChangeEvent} from "react";
import {WishlistClient, WishlistModel, WishlistSummaryModel} from "../../nswag/api.generated";

interface WishlishHomepageProps{
    wishlistClient: WishlistClient;
}

interface WishlishHomepageState{
    creating : boolean;
    nameOfNewWishlist : string;
    wishlistSummaries : WishlistSummaryModel[];
}

export class WishlistHomepage extends React.Component<WishlishHomepageProps, WishlishHomepageState>{
    constructor(props : WishlishHomepageProps) {
        super(props);
        
        this.state = {
            creating: false,
            nameOfNewWishlist: '',
            wishlistSummaries: []
        };

        this.onClickNewWishlist = this.onClickNewWishlist.bind(this);
        this.onClickCreate = this.onClickCreate.bind(this);        
        this.handleInputChange = this.handleInputChange.bind(this);
    }
    
    onClickNewWishlist = () => 
    {
        this.setState({
           creating: true
        });
    }
    
    onClickCreate = async () => 
    {
        await this.props.wishlistClient.create(this.state.nameOfNewWishlist);
        this.setState({
            creating: false
        });
        this.loadWishListSummaries();
    }

    handleInputChange(event : ChangeEvent<HTMLInputElement>) {
        this.setState({
            nameOfNewWishlist: event.target.value
        });
    }
    
    loadWishListSummaries = async () => {
        var summaries = await this.props.wishlistClient.getSummaries();
        this.setState({
            wishlistSummaries: summaries
        });
    }
    
    componentDidMount() {
        this.loadWishListSummaries();
    }

    render() {
        let createButtons;
        if(!this.state.creating){
            createButtons = <button onClick={this.onClickNewWishlist}>Create New Wishlist</button>
        }
        else{
            createButtons = <div><input type="text" value={this.state.nameOfNewWishlist} onChange={this.handleInputChange} placeholder='Name of wishlist...'></input> <button onClick={this.onClickCreate}>Create</button></div>
        }
        
        let summaries = <ul> {this.state.wishlistSummaries.map(s => <li key={s.name}>{s.name}</li>) } </ul>
        
        return (
            <div>
                { createButtons }
                { summaries }
            </div>
        );
    }
}