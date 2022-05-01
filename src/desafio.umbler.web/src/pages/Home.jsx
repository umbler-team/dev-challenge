import React, { useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import axios from 'axios';
import styles from '../styles/Home.module.scss';

function Home() {
    const [dominio, setDominio] = useState("");
    const [rows, setRows] = useState();
    const validaDominio = /^[a-zA-Z0-9-_]+[.\\]+[a-zA-Z0-9-_]+/gm;

    function renderRow () {
        if(rows) {
            return rows.map( element =>{
                    return (
                        <TableRow
                            key={element?.name}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell align="center">{element?.ip}</TableCell>
                            <TableCell align="center">{element?.name}</TableCell>
                            <TableCell align="center">{element?.hostedAt}</TableCell> 
                        </TableRow> 
                    )
                }
            )
        }
    }

    return (
        <div className={styles.home}>
            <div className={styles.searchHome}>
                <TextField 
                    id="outlined-basic"
                    label="Domínio"
                    variant="outlined"
                    value={dominio}
                    onChange={e => {
                        setDominio(e.target.value);
                    }}
                />
                <Button
                    className={styles.searchButton}
                    variant="contained"
                    onClick={e => {
                        if(validaDominio.test(dominio)) {
                            axios.get(`${process.env.REACT_APP_URL_API}api/domain/${dominio}`)
                                .then(res => {
                                    const data = res.data;
                                    setRows([{
                                        hostedAt: data.hostedAt,
                                        ip: data.ip,
                                        name: data.name,
                                        whoIs: data.whoIs
                                    }])
                                })
                        } else {
                            alert("Dominio invalido!");
                        }
                    }}
                >
                    Pesquisar
                </Button>
            </div>
            <TableContainer component={Paper}>
                <Table sx={{ minWidth: '50vw' }} size="small" aria-label="a dense table">
                    <TableHead>
                        <TableRow>
                            <TableCell align="center">Ip</TableCell>
                            <TableCell align="center">Nome</TableCell>
                            <TableCell align="center">Hospedado em</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {renderRow()}
                    </TableBody>
                </Table>
            </TableContainer>
        </div>
    )
}

export default Home;