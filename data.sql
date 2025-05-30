--
-- PostgreSQL database dump
--

-- Dumped from database version 16.8 (Debian 16.8-1.pgdg120+1)
-- Dumped by pg_dump version 17.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: database_vejr_user
--

INSERT INTO public."Users" ("Id", "Username", "PasswordHash", "FirstName", "LastName", "Email") VALUES (1, 'admin', '$2a$11$XD7iju9tvjWtvlDeqQiqjusUOT7HCsoUIyExbz7sr1cZUjx/h33mi', '', '', '');
INSERT INTO public."Users" ("Id", "Username", "PasswordHash", "FirstName", "LastName", "Email") VALUES (2, 'hemasai', '$2a$11$zawGG01eIFbBGSOnU7OBw.NkG6zpRR5yV3f.IVMmztgiq51M1mZSO', '', '', '');


--
-- Data for Name: Companies; Type: TABLE DATA; Schema: public; Owner: database_vejr_user
--



--
-- Data for Name: PurchaseOrders; Type: TABLE DATA; Schema: public; Owner: database_vejr_user
--



--
-- Data for Name: LineItems; Type: TABLE DATA; Schema: public; Owner: database_vejr_user
--



--
-- Name: Companies_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: database_vejr_user
--

SELECT pg_catalog.setval('public."Companies_Id_seq"', 1, false);


--
-- Name: LineItems_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: database_vejr_user
--

SELECT pg_catalog.setval('public."LineItems_Id_seq"', 1, false);


--
-- Name: PurchaseOrders_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: database_vejr_user
--

SELECT pg_catalog.setval('public."PurchaseOrders_Id_seq"', 1, false);


--
-- Name: Users_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: database_vejr_user
--

SELECT pg_catalog.setval('public."Users_Id_seq"', 2, true);


--
-- PostgreSQL database dump complete
--

