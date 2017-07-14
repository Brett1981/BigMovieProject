var data = '{"hello": "world"}'
//kreiramo objekt iz JSON teksta in zapišemo v spremenljivko podatki
var podatki = JSON.parse(data)
//izpišemo v konzolo vrednost ključa 'hello'
console.log("Parse izpis:",podatki.hello);
//izpišemo tekstovno vrednost objekta podatki
console.log("Stringify izpis: ",JSON.stringify(podatki))



