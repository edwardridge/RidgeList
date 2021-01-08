import React, { useState } from "react";
import {RouteComponentProps } from "react-router-dom";
import {WishlistClient, WishlistModel} from "../../nswag/api.generated";

interface WishlistProps {
    id: string;
}

interface Props extends RouteComponentProps<WishlistProps> {
}

export const Wishlist : React.FC<Props> = (props) => {
    const [wishlist, setWishlist] = useState({} as WishlistModel);

    let id = props.match.params.id;
    new WishlistClient().getWishlist(id).then(s => setWishlist(s));

    const addPerson = async () => {
        var newWishlist = await new WishlistClient().addPerson(wishlist?.id, "New person");
        setWishlist(newWishlist);
    }

    if (wishlist) {
        let listOfPeople = <ul> {wishlist.people?.map((s,i) => <li key={`${s}${i}`}>{s}</li>)}</ul>

        return (
            <div>
                <h1>Wishlist - {wishlist.name}</h1>
                <button onClick={addPerson}>Add Person</button>
                {listOfPeople}
            </div>
        )
    }

    return (
        <div>Hi</div>
    );
}

//export class Wishlist extends React.Component<Props, WishlistState>{
//    constructor(props : Props) {
//        super(props);
//        this.state = {
//            wishlist: null
//        }
//    }
    
//    async componentDidMount() {
//        var id  = this.props.match.params.id;
//        //Todo: remove wishlistclient
//        var wishlist = await new WishlistClient().getWishlist(id);
        
//        this.setState({wishlist: wishlist});
//    }
    
//    render(){
//        if(this.state.wishlist){
//            let listOfPeople = <ul> {this.state.wishlist.people?.map(s => <li>{s}</li>) }</ul>
            
//            return (
//                <div>
//                    <h1>Wishlist - {this.state.wishlist.name}</h1>
//                    <button onClick={this.addPerson}>Add Person</button>
//                    {listOfPeople}
//                </div>
//            )
//        }
//        return null;
//    }

//    addPerson = async () => {
//        var newWishlist = await new WishlistClient().addPerson(this.state.wishlist?.id, "New person");
//        this.setState({wishlist: newWishlist});
//    }
//}