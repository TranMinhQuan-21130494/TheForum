import { TextInput } from "react-native";
// import "@/global.css";

export function MyTextInput({ value, onChange, placeholder }) {
    return (<TextInput
        className="w-11/12 px-4 py-4 my-2 bg-slate-100 rounded-md"
        value={value}
        onChangeText={onChange}
        placeholder={placeholder} />);
}

export function MyPasswordInput({ value, onChange, placeholder }){
    return (<TextInput
        className="w-11/12 px-4 py-4 my-2 bg-slate-100 rounded-md"
        secureTextEntry
        value={value}
        onChangeText={onChange}
        placeholder={placeholder} />);
}