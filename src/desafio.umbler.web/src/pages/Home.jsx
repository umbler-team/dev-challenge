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
import { Box, Stack } from '@mui/material';

function Home() {
    const [dominio, setDominio] = useState("");
    const [rows, setRows] = useState();
    const validaDominio = /^[a-zA-Z0-9-_]+[.\\]+[a-zA-Z0-9-_]+/gm;

    function renderRow() {
        if (rows) {
            return (
                <TableRow
                    key={rows?.name}
                    sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                >
                    <TableCell align="center">{rows?.ip}</TableCell>
                    <TableCell align="center">{rows?.name}</TableCell>
                    <TableCell align="center">{rows?.hostedAt}</TableCell>
                </TableRow>
            )
        }
    }

    function renderWhoIs() {
        if (rows) {
            return rows.whoIs.split("\n").map((element, index) => {
                return (
                    <p>{element}</p>
                )
            })
        }
    }

    return (
        <Box
            sx={{
                display:"flex",
                alignItems:"center", 
                justifyContent:"center",
                minHeight:"100vh",
                paddingLeft:"1vw",
                paddingRight:"1vw",
                paddingTop:"1vh",
                paddingBottom:"1vh",
                flexDirection:"column"
            }}
        >
            <Stack
                spacing={2}
                direction="row"
                alignItems="center"
                height="10vh"
            >
                <TextField
                    sx={{
                        width:"60%"
                    }}
                    id="outlined-basic"
                    label="Domínio"
                    variant="outlined"
                    value={dominio}
                    onChange={e => {
                        setDominio(e.target.value);
                    }}
                />
                <Button
                    sx={{
                        maxHeight: 35,
                        width:"40%"
                    }}
                    variant="contained"
                    onClick={e => {
                        if (validaDominio.test(dominio)) {
                            axios.get(`${process.env.REACT_APP_URL_API}api/domain/${dominio}`)
                                .then(res => {
                                    const data = res.data;
                                    setRows({
                                        hostedAt: data.hostedAt,
                                        ip: data.ip,
                                        name: data.name,
                                        whoIs: data.whoIs
                                    })
                                })
                                .catch(res => {
                                    alert(res.response.data);
                                })
                        } else {
                            alert("Dominio invalido!");
                        }
                    }}
                >
                    Pesquisar
                </Button>
            </Stack>
            <TableContainer component={Paper} sx={{ minWidth: "50vw", height: "66.5" }}>
                <Table size="small">
                    <TableHead >
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
            <Box
                sx={{
                    flex: 1
                }}
            >
                {renderWhoIs()}
            </Box>
        </Box>
    )
}

export default Home;