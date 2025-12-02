--
-- PostgreSQL database dump
--

\restrict G2f120j9UIIkILYqOEY26VWGVbD18wsSSYs35HGWaLwxcDciwqoGtTdw6NWuS8o

-- Dumped from database version 18.1
-- Dumped by pg_dump version 18.1

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
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.users (id, name, age, telegram_teg, city_now, city_later, password, role) VALUES (1, 'Anton Nekhlopochin', 18, '@quasaaarx', 'Voronezsh', 'Voronezsh', '55455901', 'guest');
INSERT INTO public.users (id, name, age, telegram_teg, city_now, city_later, password, role) VALUES (2, 'Anton Nekhlopochin', 18, '@quasaaarx', 'Voronezh', 'Voronezh', NULL, NULL);
INSERT INTO public.users (id, name, age, telegram_teg, city_now, city_later, password, role) VALUES (3, 'Anton Nekhlopochin', 19, '@quasaaarx', 'Elets', 'Elets', NULL, NULL);
INSERT INTO public.users (id, name, age, telegram_teg, city_now, city_later, password, role) VALUES (4, 'Anton Nekhlopochin', 25, '2108637904', 'Voronezsh', 'Voronezsh', NULL, 'guest');
INSERT INTO public.users (id, name, age, telegram_teg, city_now, city_later, password, role) VALUES (5, 'Vyacheslav', 18, '1331310743', 'Воронеж', 'Воронеж', NULL, 'guest');
INSERT INTO public.users (id, name, age, telegram_teg, city_now, city_later, password, role) VALUES (6, 'Владимир Кузнецов', 18, '1869441303', 'Воронеж', 'Воронеж', NULL, 'guest');
INSERT INTO public.users (id, name, age, telegram_teg, city_now, city_later, password, role) VALUES (7, 'Роман', 95, '2116208057', 'Магадан', 'Магадан', NULL, 'guest');


--
-- PostgreSQL database dump complete
--

\unrestrict G2f120j9UIIkILYqOEY26VWGVbD18wsSSYs35HGWaLwxcDciwqoGtTdw6NWuS8o

