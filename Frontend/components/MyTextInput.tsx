import { TextInput } from "react-native";
// import "@/global.css";

export function MyTextInput({
    value = "",
    placeholder = "",
    onChange = (_text: string) => { },
    className = ""
}) {
    const defaultClass = "w-11/12 px-4 py-4 my-2 bg-slate-100 rounded-md";
    return (<TextInput
        className={`${defaultClass} ${className}`}
        value={value}
        onChangeText={onChange}
        placeholder={placeholder} />);
}

export function MyPasswordInput({
    value = "",
    placeholder = "",
    onChange = (_text: string) => { },
    className = ""
}) {
    const defaultClass = "w-11/12 px-4 py-4 my-2 bg-slate-100 rounded-md";
    return (<TextInput
        className={`${defaultClass} ${className}`}
        secureTextEntry
        value={value}
        onChangeText={onChange}
        placeholder={placeholder} />);
}